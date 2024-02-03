import React from 'react'

const ProjectSearchParam = (props) => {
    const {
        onRemove = () => { },
        children
    } = props;

    return (
        <div className='bg-indigo-100 rounded-full h-fit px-4 py-2 flex items-center gap-x-2 font-semibold text-sm uppercase tracking-wide text-indigo-600'>
            {children}

            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-5 h-5" onClick={onRemove}>
                <path d="M6.28 5.22a.75.75 0 0 0-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 1 0 1.06 1.06L10 11.06l3.72 3.72a.75.75 0 1 0 1.06-1.06L11.06 10l3.72-3.72a.75.75 0 0 0-1.06-1.06L10 8.94 6.28 5.22Z" />
            </svg>

        </div>
    )
}

export default ProjectSearchParam