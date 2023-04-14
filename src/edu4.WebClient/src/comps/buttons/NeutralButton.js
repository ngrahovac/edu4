import React from 'react'

const NeutralButton = (props) => {
    const {
        text,
        onClick,
        disabled
    } = props;

    return (
        <button
            onClick={onClick}
            className={`${disabled ? 'bg-gray-200 text-gray-500' : ''} py-2 px-4 rounded-full font-semibold`}
            disabled={disabled}>
            {text}
        </button>
    )
}

export default NeutralButton