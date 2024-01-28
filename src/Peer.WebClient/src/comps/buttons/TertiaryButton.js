import React from 'react'
import ButtonBase from './ButtonBase'

const TertiaryButton = (props) => {
    return (
        <ButtonBase
            {...props}
            defaultStyle="text-indigo-500"
            hoverStyle="text-indigo-800">
        </ButtonBase>
    )
}

export default TertiaryButton