import React from 'react'
import ButtonBase from './ButtonBase';

const BorderlessButton = (props) => {
    
    return (
        <ButtonBase
            {...props}
            hoverStyle="text-blue-500">
        </ButtonBase>
    )
}

export default BorderlessButton