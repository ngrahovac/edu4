import React from 'react'
import ButtonBase from './ButtonBase'

const TertiaryButton = (props) => {
    return (
        <ButtonBase
            {...props}
            defaultStyle="text-indigo-500"
            hoverStyle="hover:text-indigo-600">
        </ButtonBase>
    )
}

export default TertiaryButton