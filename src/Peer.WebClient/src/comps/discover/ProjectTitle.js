import React from 'react'

const ProjectTitle = (props) => {
    const {
        children
    } = props;

    return (
        <p className='text-2xl font-semibold text-gray-800'>{children}</p>
    )
}

export default ProjectTitle