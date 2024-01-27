import React from 'react'

const Collaborators = (props) => {
    const {
        children
    } = props;

  return (
    <div className='flex flex-wrap gap-x-12 gap-y-2'>
        {children}
    </div>
  )
}

export default Collaborators