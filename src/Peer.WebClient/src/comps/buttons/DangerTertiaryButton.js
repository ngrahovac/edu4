import React from 'react'
import ButtonBase from './ButtonBase'

const DangerTertiaryButton = (props) => {
    return (
        <ButtonBase
            {...props}
            defaultStyle="text-red-500"
            hoverStyle="text-red-800">
        </ButtonBase>
    )
}

export default DangerTertiaryButton