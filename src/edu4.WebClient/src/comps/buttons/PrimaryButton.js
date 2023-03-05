import React from 'react'

const PrimaryButton = (props) => {
    const {
        text,
        onClick,
        disabled
    } = props;

    return (
        <button
            onClick={onClick}
            className='bg-indigo-500 py-2 px-4 rounded-full font-semibold text-gray-50'>
            {text}
        </button>
    )
}

export default PrimaryButton