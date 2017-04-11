<#
.DESCRIPTION
This DSC resource provides for management of NetApp Cluster disks. It supports the creation, management and removal of the following items:

- Volumes
- LUNs
- IGroups
- Local disks
- Partitions

When the Ensure parameter is specified as Present, then the following actions are performed:

- If the volume does not exist, it is created
- If the LUN does not exist, it is created
- If the IGroup for this server does not exist, is it created
- If the server's initiators are not present in the IGroup, they are added
- If the IGroup is not mapped to the LUN, they are mapped
- If the LUN is not the correct size, it is resized
- If this volume needs to be resized to accomodate the LUN resizing, then the volume is resized
- The local disk representing the LUN is set to online and read only is removed
- If the local disk has not been initialized, it is initialized as GPT
- If there is no partition on the local disk, it is created
- If the mount points are not properly assigned to the local disk, they are updated

When the Ensure parameter is specified as Absent, then the following actions are performed:

- The local disk is set to Offline
- If the RemoveLUN parameter is specified, then the LUN is removed
- If the RemoveVolume parameter is specified, then the volume is removed

.PARAMETER Path
The path of the LUN to be created. It must be specified in the following format:

/vol/[VolumeName]/[LUN]

.PARAMETER Controller
The administrative controller of the NetApp cluster.

.PARAMETER VServer
The Data VServer of the NetApp cluster.

.PARAMETER Aggregate
The NetApp aggregate that this volume should belong to. This parameter is only used when creating new volumes. If this is applied to an existing volume, it does not verify that the volume is in the correct aggregate.

.PARAMETER MountPoint
The mount point(s) that this drive should use. Multiple options can be specified using a comma-separated format, i.e.

-MountPoint 'X, C:\TestMount'

Note: Only one drive letter may be assigned.

.PARAMETER Size
The desired size of the drive. This may be specified in bytes, or by using size modifiers, such as 100MB.

.PARAMETER IGroup
The name of the IGroup that will be used to grant access for this server to the LUN. If no IGroup name is specified, it defaults to the domain-specific FQDN of the server (i.e. Server1.domain.local)

.PARAMETER DiskType
The type of LUN to provision. The default value of 'Dedicated' should be used unless a cluster shared disk is needed. This parameter is only used for LUN creation. If this is applied to an existing LUN, the disk type is not enforced.

.PARAMETER SpaceReserve
Whether NetApp Space Reserve should be enabled on this LUN. This parameter is only used for LUN creation. If this is applied to an existing LUN, it is not enforced.

.PARAMETER Connectivity
Whether the connection to the NetApp SAN is provided via iSCSI or Fiber Channel.

.PARAMETER Credential
The credentials used to connect to the NetApp controller.

.PARAMETER RemoveVolume
When used with an 'Absent' value for the Ensure parameter, this will remove the NetApp volume associated with this disk.

Note: Other LUNs or data may be present on this volume, which would be lost if this parameter is used. This parameter should be used with caution.

.PARAMETER RemoveLUN
When used with an 'Absent' value for the Ensure parameter, this will remove the NetApp LUN associated with this disk.

.PARAMETER Ensure
Whether the disk should be present or absent.

.NOTES
Dependency: NetApp DataONTAP Module
#>
function Get-TargetResource
{
	[CmdletBinding()]
	[OutputType([System.Collections.Hashtable])]
	param
    (
        [parameter(Position = 0, Mandatory = $true)]
        [string]
        $Path,
        
        [parameter(Mandatory = $true)]
        [string]
        $Controller,
        
        [parameter(Mandatory = $true)]
        [string]
        $VServer,

        [string]
        $Aggregate,
        
        [string]
        $MountPoint,
        
        [string]
        $Size,
        
        [string]
        $IGroup,
        
        [ValidateSet("Dedicated", "Shared")]
        [string]
        $DiskType = "Dedicated",
        
        [bool]
        $SpaceReserve,

        [string]
        $SnapshotPolicy,

        [ValidateSet("iSCSI", "FiberChannel")]
        [string]
        $Connectivity,

        [PSCredential]
        $Credential,

        [bool]
        $RemoveVolume,

        [bool]
        $RemoveLun,

        [ValidateSet("Present", "Absent")]
        [string]
        $Ensure = 'Present'
    )
    
    throw "Get-TargetResource has not been implemented."    
}

function Test-TargetResource
{
	[CmdletBinding()]
	[OutputType([System.Boolean])]
	param
    (
        [parameter(Position = 0, Mandatory = $true)]
        [string]
        $Path,
        
        [parameter(Mandatory = $true)]
        [string]
        $Controller,
        
        [parameter(Mandatory = $true)]
        [string]
        $VServer,

        [string]
        $Aggregate,
        
        [string]
        $MountPoint,
        
        [string]
        $Size,
        
        [string]
        $IGroup,
        
        [ValidateSet("Dedicated", "Shared")]
        [string]
        $DiskType = "Dedicated",
        
        [bool]
        $SpaceReserve,

        [string]
        $SnapshotPolicy,

        [ValidateSet("iSCSI", "FiberChannel")]
        [string]
        $Connectivity,

        [PSCredential]
        $Credential,

        [bool]
        $RemoveVolume,

        [bool]
        $RemoveLun,

        [ValidateSet("Present", "Absent")]
        [string]
        $Ensure = 'Present'
    )

    #region Initialize
    $Path = $Path.ToLower()
    if (-not $Path.StartsWith("/vol/"))
    {
        throw "The path must specify a storage system path (i.e. /vol/<Volume>/<Lun>.lun"
    }

    if ($global:CurrentNcController -eq $null)
    {
        $c = Connect-NcController -Name $Controller -Vserver $VServer -Credential $Credential
        if ($c -eq $null)
        {
            throw "Could not connect to controller."
        }
    }

    $TestResults = @{}   

    # Gather information
    if ($Connectivity -eq "iSCSI")
    {
        [array]$initiator = (Get-InitiatorPort -ConnectionType iSCSI).NodeAddress
    }
    else
    {
        [array]$initiator = (Get-InitiatorPort -ConnectionType FibreChannel).PortAddress
    }
    if ([string]::IsNullOrEmpty($IGroup))
    {
        # Set default IGroup name
        $IGroup = [System.Net.Dns]::GetHostEntry([string]"localhost").HostName.ToLower()
    }

    $volumeName = Split-Path -Path (Split-Path -Path $Path -Parent) -Leaf
    $lunName = Split-Path -Path $Path -Leaf
    
    $naLun = Get-NcLun $Path -ErrorAction SilentlyContinue
    $naVol = Get-NcVol -Name $volumeName -ErrorAction SilentlyContinue
    $naIgroup = Get-NcIgroup -Name $IGroup -ErrorAction SilentlyContinue
    $lunMap = Get-NcLunMap -Path $Path -ErrorAction SilentlyContinue | Where-Object { $_.InitiatorGroup -eq $IGroup }

    $disk = Get-Disk | Where-Object { $_.SerialNumber -eq $naLun.SerialNumber }
    $partition = $disk | Get-Partition -ErrorAction SilentlyContinue | Where-Object { $_.Type -ne "Reserved" } | Select -First 1

    if (-not [string]::IsNullOrEmpty($MountPoint))
    {
        $MountPoints = Parse-MountPoints -MountPoints $MountPoint
    }

    # Set default values for unspecified parameters

    if ([string]::IsNullOrEmpty($Size) -and $naLun.Size -gt 0)
    {
        $Size = $naLun.Size
        $NoResize = $true
    }

    $targetLunSize = Convert-StringToBytes $Size
    $targetVolSize = $targetLunSize * 1.1

    #endregion

    # Begin Absent tests
    if ($Ensure -eq 'Absent')
    {
        if (-not $disk.IsOffline)
        {
            Write-DscLog "The disk should be disconnected."
            $TestResults.Add('DisconnectLun', $true)
        }

        if ($RemoveLun -and $naLun -ne $null)
        {
            Write-DscLog "The LUN is detected and set for removal. It will be removed."
            $TestResults.Add('RemoveLun', $true)
        }

        if ($RemoveVolume -and $naVol -ne $null)
        {
            Write-DscLog "The volume is detected and set for removal. It will be removed."
            $TestResults.Add('RemoveVolume', $true)
        }
        $Script:Results = $TestResults
        return
    }

    # Start tests
    if ($disk -eq $null -and $naLun -ne $null)
    {
        Write-DscLog "LUN exists but is not connected. It will be connected to this client."
        $TestResults.Add('ConnectLun', $true)
    }

    if ($disk.IsOffline -or $disk.IsReadOnly -or $disk.PartitionStyle -eq 'RAW')
    {
        Write-DscLog "The LUN is connected but is not ready for use. It will be updated."
        $TestResults.Add('PrepareLun', $true)
    }

    if ($disk -ne $null -and $partition -eq $null)
    {
        Write-DscLog "The partition does not exist. It will be created."
        $TestResults.Add('CreatePartition', $true)
    }

    if ($lunMap -eq $null)
    {
        Write-DscLog "The lun mapping does not exist. It will be created."
        $TestResults.Add('CreateLunMap', $true)
    }

    if ($naLun -eq $null)
    {
        Write-DscLog "LUN does not exist. It will be created."
        $TestResults.Add('CreateLun', $true)
    }

    if ($naVol -eq $null)
    {
        Write-DscLog "Volume does not exist. It will be created."
        $TestResults.Add('CreateVolume', $true)
    }

    if ($naIgroup -eq $null)
    {
        Write-DscLog "IGroup does not exist. It will be created."
        $TestResults.Add('CreateIgroup', $true)
    }

    if ($naIgroup -ne $null -and ($naIgroup.Initiators | Where-Object { $initiator -contains $_.InitiatorName }) -eq $null)
    {
        $toAdd = $false
        foreach($init in $initiator)
        {
            if ($Connectivity -eq 'FiberChannel')
            {
                $initTest = $init -replace "(\w{2})", "`$`1:"
                $initTest = $initTest.Remove($initTest.Length - 1)
            }

            if ($naIGroup.Initiators.InitiatorName -notcontains $initTest)
            {
                Write-DscLog "The IGroup $IGroup exists, but does not contain the initiator: $initTest. It will be added."
                $toAdd = $true
            }
        }
        
        if ($toAdd)
        {
            $TestResults.Add('AddInitiator', $true)
        }
    }
    
    if (-not $TestResults.ContainsKey("CreateLun") -and $NoResize -ne $true -and [Math]::Abs($targetLunSize - $naLun.Size) -gt ($naLun.Size * .025))
    {
        Write-DscLog "LUN Resize is needed. It is currently $([Math]::Ceiling($naLun.Size / 1mb))MB, but has been specified to be $([Math]::Ceiling($targetLunSize / 1mb))MB."
        $TestResults.Add('ResizeLun', $true)
    }

    if (-not $TestResults.ContainsKey("CreateVolume") -and $TestResults.ContainsKey('ResizeLun'))
    {
        $delta = $targetLunSize - $naLun.Size
        if ($naVol.Available -lt $delta * 1.1)
        {
            $targetVolSize = $naVol.Size + $delta
            Write-DscLog "The volume needs to be resized. It is currently $($naVol.Size / 1mb)MB and needs to be $($targetVolSize / 1mb)MB."
            $TestResults.Add('ResizeVolume', $true)
        }
    }


    if ($partition.AccessPaths -ne $null)
    {
        $existingMp = Parse-MountPoints -MountPoints ($partition.AccessPaths -join ", ")
        if ((-not [string]::IsNullOrEmpty($MountPoint) -and -not (CompareMountPoints $MountPoints ($existingMp))))
        {
        
            Write-DscLog "The mount points for this drive need to be updated."
            $TestResults.Add('UpdateMountPoint', $true)
        }
    }

    $Script:Results = $TestResults
    return $TestResults.Count -eq 0
}

function Set-TargetResource
{
	[CmdletBinding(SupportsShouldProcess = $true)]
	param
    (
        [parameter(Position = 0, Mandatory = $true)]
        [string]
        $Path,
        
        [parameter(Mandatory = $true)]
        [string]
        $Controller,
        
        [parameter(Mandatory = $true)]
        [string]
        $VServer,

        [string]
        $Aggregate,
        
        [string]
        $MountPoint,
        
        [string]
        $Size,
        
        [string]
        $IGroup,
        
        [ValidateSet("Dedicated", "Shared")]
        [string]
        $DiskType = "Dedicated",
        
        [bool]
        $SpaceReserve,

        [string]
        $SnapshotPolicy,

        [ValidateSet("iSCSI", "FiberChannel")]
        [string]
        $Connectivity,

        [PSCredential]
        $Credential,

        [bool]
        $RemoveVolume,

        [bool]
        $RemoveLun,

        [ValidateSet("Present", "Absent")]
        [string]
        $Ensure = 'Present'
    )

    #region Initialize
    $Path = $Path.ToLower()
    if (-not $Path.StartsWith("/vol/"))
    {
        throw "The path must specify a storage system path (i.e. /vol/<Volume>/<Lun>.lun"
    }

    if ($global:CurrentNcController.Name -ne $Controller)
    {
        $c = Connect-NcController -Name $Controller -Vserver $VServer -Credential $Credential
        if ($c -eq $null)
        {
            throw "Could not connect to controller."
        }
    }

    # Gather information
    if ($Connectivity -eq "iSCSI")
    {
        [array]$initiator = (Get-InitiatorPort -ConnectionType iSCSI).NodeAddress
    }
    else
    {
        [array]$initiator = (Get-InitiatorPort -ConnectionType FibreChannel).PortAddress
    }
    if ([string]::IsNullOrEmpty($IGroup))
    {
        #$IGroup = "viaRPC.$($initiator[0])"
        # Set default IGroup name
        $IGroup = [System.Net.Dns]::GetHostEntry([string]"localhost").HostName.ToLower()
    }

    $volumeName = Split-Path -Path (Split-Path -Path $Path -Parent) -Leaf
    $lunName = Split-Path -Path $Path -Leaf
    
    $naLun = Get-NcLun $Path -ErrorAction SilentlyContinue
    $naVol = Get-NcVol -Name $volumeName -ErrorAction SilentlyContinue
    $naIgroup = Get-NcIgroup -Name $IGroup -ErrorAction SilentlyContinue
    $lunMap = Get-NcLunMap -Path $Path -ErrorAction SilentlyContinue | Where-Object { $_.InitiatorGroup -eq $IGroup }
    
    IF ($naLun.SerialNumber -ne $null)
    {
        $disk = Get-Disk | Where-Object { $_.SerialNumber -eq $naLun.SerialNumber }
        $partition = $disk | Get-Partition -ErrorAction SilentlyContinue | Where-Object { $_.Type -ne "Reserved" } | Select -First 1
    }
    
    if (-not [string]::IsNullOrEmpty($MountPoint))
    {
        $MountPoints = Parse-MountPoints -MountPoints $MountPoint
    }

    # Set default values for unspecified parameters

    if ([string]::IsNullOrEmpty($Size) -and $naLun.Size -gt 0)
    {
        $Size = $naLun.Size
        $NoResize = $true
    }

    $targetLunSize = Convert-StringToBytes $Size
    $targetVolSize = $targetLunSize * 1.1
    #endregion
    
    if ($Ensure -eq 'Absent')
    {
        if ($Script:Results['DisconnectLun'])
        {
            if ($PSCmdlet.ShouldProcess($Path, "Disconnect LUN"))
            {
                $disk | Set-Disk -IsOffline $true
            }
        }

        if ($Script:Results['RemoveLun'])
        {
            if ($PSCmdlet.ShouldProcess($Path, "Remove LUN"))
            {
                Remove-NcLun -Path $Path -Force | Out-Null
            }
        }

        if ($Script:Results['RemoveVolume'])
        {
            if ($PSCmdlet.ShouldProcess($volumeName, "Remove volume"))
            {
                if ($naVol.State -ne 'offline')
                {
                    Dismount-NcVol -Name $volumeName -Force | Out-Null
                    Set-NcVol -Name $volumeName -Offline | Out-Null
                }
                Remove-NcVol -Name $volumeName | Out-Null
            }
        }
        return
    }

    if ($Script:Results['CreateVolume'])
    {
        $aggregateAvailable = Get-NcAggr
        if ($aggregateAvailable.Name -notcontains $Aggregate)
        {
            $Aggregate = ($aggregateAvailable | Sort-Object Available -Descending | Select-Object -First 1).Name
        }
        $params  = @{}
        if (-not $SpaceReserve)
        {
            $params.Add("SpaceReserve", "none")
        }

        if (-not [string]::IsNullOrEmpty($SnapshotPolicy))
        {
            $params.Add('SnapshotPolicy', $SnapshotPolicy)
        }

        if ($PSCmdlet.ShouldProcess("/vol/$($volumeName)", "Create volume"))
        {
            
            New-NcVol -Name $volumeName -SnapshotReserve 0 -Size $targetVolSize -Aggregate $Aggregate -JunctionPath "/$($volumeName)" -ErrorAction Stop @params | Out-Null
        }
    }

    if ($Script:Results['CreateLun'])
    {
        if ($PSCmdlet.ShouldProcess($Path, "Create LUN"))
        {
            $params = @{}
            if (-not $SpaceReserve)
            {
                $params.Add('Unreserved', $true)
            }

            $naLun = New-NcLun -Path $Path -OsType 'windows_2008' -QoSPolicyGroup Unlimited -Size (Convert-SizeToNetAppSize $targetLunSize) @params
            Start-Sleep -Seconds 3

            $Script:Results['PrepareLun'] = $true
        }
    }

    if ($Script:Results['ResizeVolume'])
    {
        if ($PSCmdlet.ShouldProcess($targetVolSize, "Resize Volume"))
        {
            $delta = $targetLunSize - $naLun.Size
            $targetVolSize = $naVol.Size + $delta
            Set-NcVolSize -Name $volumeName -NewSize $targetVolSize -ErrorAction Stop | Out-Null
        }
    }

    if ($Script:Results['CreateIgroup'])
    {
        if ($PSCmdlet.ShouldProcess("Igroup $IGroup", "Create Igroup"))
        {
            
            if ($Connectivity -eq 'iSCSI')
            {
                New-NcIgroup -Name $IGroup -Protocol "iscsi" -Type "windows" -ErrorAction Stop | Out-Null
            }
            else
            {
                New-NcIgroup -Name $IGroup -Protocol 'fcp' -Type "windows" -ErrorAction Stop | Out-Null
            }
            $Script:Results['AddInitiator'] = $true
        }
    }

    if ($Script:Results['AddInitiator'])
    {
        if ($PSCmdlet.ShouldProcess("Igroup $IGroup", "Add initiator $iniator"))
        {
            $initiator | ForEach-Object { $outnull = Add-NcIgroupInitiator -Igroup $IGroup -Initiator $_ -ErrorAction Stop }
        }
    }

    if ($Script:Results['ResizeLun'])
    {
        if ($targetLunSize -lt $naLun.Size)
        {
            $isShrink = $true
        }

        if ($PSCmdlet.ShouldProcess($lunName, "Resizing LUN"))
        {
            $lun = Set-NcLunSize -Path $Path -NewSize (Convert-SizeToNetAppSize $targetLunSize) -Force
            Update-HostStorageCache
            $sizeMax = ($partition | Get-PartitionSupportedSize).SizeMax
            $sizeMin = ($partition | Get-PartitionSupportedSize).SizeMin
            if ($isShrink)
            {
                Write-DscLog "For shrinking, resizing to $sizeMin. (max $sizeMax)"
                $partition | Resize-Partition -Size $sizeMin
                Update-HostStorageCache
                $sizeMax = ($partition | Get-PartitionSupportedSize).SizeMax
            }
            
            Write-DscLog "Resizing to $sizeMax."
            $partition | Resize-Partition -Size $sizeMax
        }
    }

    if ($Script:Results['CreateLunMap'])
    {
        if ($PSCmdlet.ShouldProcess($Path, "Create LUN mapping"))
        {
            Add-NcLunMap -Path $Path -InitiatorGroup $IGroup | Out-Null
        }
    }

    if ($Script:Results['ConnectLun'])
    {
        throw "The LUN does not appear to be connected to the target machine."
    }

    if ($Script:Results['PrepareLun'])
    {
        if ($PSCmdlet.ShouldProcess($Path, "Prepare LUN"))
        {
            Update-HostStorageCache
            $disk = Get-Disk | Where-Object { $_.SerialNumber -eq $naLun.SerialNumber }

            if ($disk.Count -gt 1)
            {
                Write-DscLog 'Multiple drives detected, MPIO is not yet being used. Rebooting machine...'
                $global:DSCMachineStatus = 1
                return
            }

            if ($disk -eq $null)
            {
                throw "The specified disk is not connected to this machine."
            }

            if ($disk.IsOffline)
            {
                $disk | Set-Disk -IsOffline $false -ErrorAction SilentlyContinue
                if (-not $? -and $Error[0].Message -match 'The media is write protected')
                {
                    Write-DscLog 'Rebooting machine...'
                    $global:DSCMachineStatus = 1
                    return
                }
            }

            if ($disk.IsReadOnly)
            {
                $disk | Set-Disk -IsReadOnly $false -ErrorAction SilentlyContinue
                if (-not $? -and $Error[0].Message -match 'The media is write protected')
                {
                    Write-DscLog 'Rebooting machine...'
                    $global:DSCMachineStatus = 1
                    return
                }
            }

            if ($disk.PartitionStyle -eq "RAW")
            {
                Write-DscLog "This disk has not been initialized. It will be initialized now."
                $disk | Initialize-Disk -PartitionStyle 'GPT' -Confirm:$false
            }
            $Script:Results['CreatePartition'] = $true
        }
    }

    if ($Script:Results['CreatePartition'])
    {
        if ($PSCmdlet.ShouldProcess($Path, "Create Partition"))
        {
            $partition = $disk | Get-Partition | Where-Object { $_.Type -ne "Reserved" } | Select -First 1
            if ($partition -eq $null)
            {
                Write-DscLog "There is no partition on the disk. It will be created."
                $partitionParams = @{}
                $partition = $disk | New-Partition -UseMaximumSize
                $partition | Format-Volume -FileSystem NTFS -Confirm:$false -NewFileSystemLabel $lunName -Force | Out-Null
            }
            $Script:Results['UpdateMountPoint'] = $true
        }
    }

    if ($Script:Results['UpdateMountPoint'])
    {
        if ($PSCmdlet.ShouldProcess("$MountPoint", "Update mount points"))
        {
            $mp = Parse-MountPoints -MountPoints $MountPoint
            $accessPath = Parse-MountPoints -MountPoints ($partition.AccessPaths -join ", ")

            foreach($mpp in $accessPath.MountPoints)
            {
                $partition | Remove-PartitionAccessPath -AccessPath $mpp
                Remove-Item -Path $mpp -Force
            }

            if ($accessPath.Drive)
            {
                $partition | Remove-PartitionAccessPath -AccessPath "$($accessPath.Drive):"
            }

            if ($mp.Drive -ne $null)
            {
                $partition | Add-PartitionAccessPath -AccessPath "$($mp.Drive):"
            }

            foreach($mpp in $mp.MountPoints)
            {
                if (-not (Test-Path $mpp))
                {
                    New-Item -Path $mpp -ItemType Directory -Force | Out-Null
                }
                $partition | Add-PartitionAccessPath -AccessPath $mpp
            }
        }
    }
}

Export-ModuleMember Get-TargetResource
Export-ModuleMember Test-TargetResource
Export-ModuleMember Set-TargetResource

#region Misc functions
function Convert-StringToBytes
{
    param
    (
        [parameter(Mandatory = $true, Position = 0)]
        [string]$Size
    )

    $Size = $Size.Replace(" ", "")
    if ($Size -match "(?i)(\d+[\.\d+]?)(B|KB|MB|GB|TB|PB)?")
    {
        if ($Matches[2] -ne "B")
        {
            $denominator = "1$($Matches[2])"
            return [uint64]$Matches[1] * $denominator
        }
        return $Matches[1]
    }
    else
    {
        Write-Error "The string was not in a recognizable format."
    }
}

function Convert-SizeToNetAppSize
{
    param
    (
        [parameter(Mandatory = $true, Position = 0)]
        $Size
    )

    $Size = Convert-StringToBytes $Size
    return "$($Size / 1mb)MB"
}

function Parse-MountPoints
{
    param
    (
        [parameter(Mandatory = $true, Position = 0, ParameterSetName = 'string')]
        [string]$MountPoints,
        [parameter(Mandatory = $true, Position = 0, ParameterSetName = 'array')]
        [array]$MountPointArray
    )

    if ($PSCmdlet.ParameterSetName -eq 'array')
    {
        $MountPoints = $MountPointArray -join ", "
    }

    $returnValue = New-Object PSObject -Property @{"Drive" = $null; "MountPoints" = @()}
    $mounts = $MountPoints -split ", "
    foreach($mount in $mounts)
    {
        if ($mount.StartsWith("\\?\Volume"))
        {
            continue
        }

        if($mount -match "^([A-Za-z]):?\\?$")
        {
            if (-not [string]::IsNullOrEmpty($returnValue.Drive))
            {
                throw "The specified mount points include more than one drive assignment, which is not permitted."
            }
            $returnValue.Drive = $Matches[1]
        }
        else
        {
            $returnValue.MountPoints += $mount
        }
    }

    $returnValue | Add-Member -MemberType ScriptMethod -Name ToString -Value { 
            $list = @()
            if (-not [string]::IsNullOrEmpty($this.Drive))
            {
                $list += "$($this.Drive):\"
            }
            $list += $this.MountPoints
            $list -join ", "
        } -Force
    return $returnValue
}

function CompareMountPoints
{
    param
    (
        [parameter(Mandatory = $true, Position = 0)]
        $OriginalMountPoint,
        [parameter(Mandatory = $true, Position = 1)]
        $ComparisonMountPoint
    )

    if ($OriginalMountPoint.Drive -ne $ComparisonMountPoint.Drive)
    {
        return $false
    }

    if ($OriginalMountPoint.MountPoints.Count -ne $ComparisonMountPoint.MountPoints.Count)
    {
        return $false
    }

    for($i = 0; $i -lt $OriginalMountPoint.MountPoints.Count; $i++)
    {
        if ($OriginalMountPoint.MountPoints[$i] -ne $ComparisonMountPoint.MountPoints[$i])
        {
            return $false
        }
    }
    return $true
}

#endregion
