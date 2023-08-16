import React from 'react'
import ButtonBase from './ButtonBase';

const NeutralButton = (props) => {
    
    return (
        <ButtonBase
            {...props}
            defaultStyle="border border-gray-500"
            hoverStyle="bg-gray-100">
        </ButtonBase>
    )
}

export default NeutralButton