import React from 'react'

const Collaborators = (props) => {
    const {
        children
    } = props;

  return (
    <div className='flex flex-row flex-wrap space-x-12'>
        {children}
    </div>
  )
}

export default Collaborators