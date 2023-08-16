import React from 'react'
import ButtonBase from './ButtonBase';

const DangerButton = (props) => {

    return (
        <ButtonBase
            {...props}
            defaultStyle="bg-red-500 text-gray-50"
            hoverStyle="bg-red-600">
        </ButtonBase>
    )
}
export default DangerButton