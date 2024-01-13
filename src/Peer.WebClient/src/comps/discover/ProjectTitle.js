import React from 'react'

const ProjectTitle = (props) => {
    const {
        children
    } = props;

    return (
        <p className='text-2xl font-semibold text-indigo-500'>{children}</p>
    )
}

export default ProjectTitle