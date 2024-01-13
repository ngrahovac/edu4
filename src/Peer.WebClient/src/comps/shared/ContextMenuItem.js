import React from 'react'

const ContextMenuItem = (props) => {
    const {
        children,
        onClick = () => {}
    } = props;

  return (
    <div 
        onClick={onClick}
        className='text-md font-medium px-4 py-1 cursor-pointer'>
        {children}
    </div>
  )
}

export default ContextMenuItem