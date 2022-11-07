import React from 'react'

const VisibilityFlair = ({ text }) => {
    return (
        <label
            className='inline-flex items-center text-xs m-2 px-2 font-extralight outline outline-1 rounded-full outline-slate-500 text-slate-500 max-h-4 whitespace-nowrap'>
            <span>{text}</span>
        </label>
    )
}

export default VisibilityFlair