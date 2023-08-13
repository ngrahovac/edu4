import React from 'react'
import ButtonBase from './ButtonBase';

const PrimaryButton = (props) => {
    
    return (
        <ButtonBase
            {...props}
            defaultStyle="bg-indigo-500 text-gray-50"
            hoverStyle="bg-indigo-600">
        </ButtonBase>
    )
}

export default PrimaryButton