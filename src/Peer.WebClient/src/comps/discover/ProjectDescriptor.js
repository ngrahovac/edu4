import React from 'react'

const ProjectDescriptor = (props) => {
  const {
    icon,
    value,
    link
  } = props;

  return (
    <div className="flex items-center shrink-0 gap-x-2">
      {icon}
      <div className='font-semibold text-sm uppercase tracking-wide text-gray-400'>{value}</div>
    </div>
  )
}

export default ProjectDescriptor