const get = u => fetch(u).then(r => r.json())

const afterLoadBootstrap = (data) => ({
  type: 'LOAD_BOOTSTRAP',
  data
})

const loadBootstrap = (node) => async(dispatch) => {
  let data = await get("https://dsc-dev.winsys.tmcs/api/v2/bootstrap/")
  dispatch(afterLoadBootstrap(data))
}

export default loadBootstrap 
