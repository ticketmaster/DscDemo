import React, {PropTypes} from 'react'
import Jumbotron from 'react-bootstrap/lib/Jumbotron';
import Button from 'react-bootstrap/lib/Button';

const url = 'https://dsc-dev.winsys.tmcs/api/v2/bootstrap/rdsh99arcphx2'

const Spinner = ({ loadBootstrap }) => (
    <Button bsStyle="primary" onClick={() => loadBootstrap('rdsh1arcash2')}>Load bootstrap data ...</Button>
)

const List = (props) => (
  <div>
    <pre>{JSON.stringify(props.data, null, 2)}</pre>
  </div>
)

const BootstrapInformation = (props) => (
  props.data.NodeName ? <div>{props.data.NodeName}</div> : <Spinner {...props}/>
)

export default BootstrapInformation
