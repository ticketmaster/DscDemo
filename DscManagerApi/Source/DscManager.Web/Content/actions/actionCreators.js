const get = u => fetch(u).then(r => r.json())

const afterLoad = (data) => ({
  type: 'LOAD_DATA',
  data
})

export const load = (url) => async(dispatch) => {
  let data = await get(url)
  dispatch(afterLoad(data))
}

const afterLoadBootstrap = (data) => ({
  type: 'LOAD_BOOTSTRAP',
  data
})

export const loadBootstrap = (node) => async(dispatch) => {
  let data = await get("http://localhost/api/v2/bootstrap/" + node)
  dispatch(afterLoadBootstrap(data))
}
