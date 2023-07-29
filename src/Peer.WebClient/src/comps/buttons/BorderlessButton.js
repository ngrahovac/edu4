import React from 'react'

const BorderlessButton = (props) => {
    const {
        text,
        onClick
    } = props;

    return (
        <button
            onClick={onClick}
            className='font-semibold flex flex-row items-center shrink-0'>
            {text}
        </button>
    )
}

export default BorderlessButton