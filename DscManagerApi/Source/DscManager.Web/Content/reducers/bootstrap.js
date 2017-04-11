const bootstrapLoaded = (state = [], action) => {
  switch(action.type) {
    case 'LOAD_BOOTSTRAP':
      return action.data
    default:
      return state
  }
}

export default bootstrapLoaded
