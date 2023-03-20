import React from 'react'

const ProjectDescriptor = (props) => {
    const {
        icon,
        value
    } = props;

  return (
    <div className='flex flex-row shrink-0 items-center'>
        <div className='mr-1'>{icon}</div>        
        <p>{value}</p>
    </div>
  )
}

export default ProjectDescriptor