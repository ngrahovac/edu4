import React from 'react'
import ButtonBase from './ButtonBase'

const AccentButton = (props) => {
    return (
        <ButtonBase
            {...props}
            defaultStyle="bg-gray-500 text-gray-50"
            hoverStyle="bg-gray-600">
        </ButtonBase>
    )
}

export default AccentButton