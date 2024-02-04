import React from 'react'
import ButtonBase from './ButtonBase'

const AccentButton = (props) => {
    return (
        <ButtonBase
            {...props}
            defaultStyle="bg-lime-500 text-gray-50"
            hoverStyle="bg-lime-600">
        </ButtonBase>
    )
}

export default AccentButton