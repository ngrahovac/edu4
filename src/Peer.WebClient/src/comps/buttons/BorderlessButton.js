import React from 'react'
import ButtonBase from './ButtonBase';

const BorderlessButton = (props) => {
    
    return (
        <ButtonBase
            {...props}
            defaultStyle="text-indigo-500 px-0"
            hoverStyle="hover:text-indigo-600">
        </ButtonBase>
    )
}

export default BorderlessButton