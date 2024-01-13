import React from 'react'

const ContributionsMenuItem = (props) => {
    const {
        onClick = () => {},
        children
    } = props;

  return (
    <div 
        onClick={onClick}
        className='text-lg font-medium px-8 py-3 cursor-pointer'>
        {children}
    </div>
  )
}

export default ContributionsMenuItem