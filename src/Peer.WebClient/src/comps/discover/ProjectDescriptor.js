import React from 'react'

const ProjectDescriptor = (props) => {
  const {
    icon,
    value,
    link
  } = props;

  return (
    <div className={`flex flex-row shrink-0 items-center`}>
      <div className='mr-1'>{icon}</div>
      <p className={`${link ? "text-blue-500 underline hover:text-blue-900" : ""}`}>{value}</p>
    </div>
  )
}

export default ProjectDescriptor