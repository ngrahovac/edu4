import React from 'react'

const BorderlessButtonWithIcon = (props) => {
    const {
        icon,
        text,
        onClick
    } = props;

    return (
        <button
            onClick={onClick}
            className='font-semibold flex flex-row items-center shrink-0'>          
            <div className='mr-2'>{icon}</div>              
            {text}
        </button>
    )
}

export default BorderlessButtonWithIcon