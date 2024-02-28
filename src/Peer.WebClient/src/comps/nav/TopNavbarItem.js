import React from 'react'
import ButtonBase from '../buttons/ButtonBase';

const TopNavbarItem = (props) => {

    return (
        <ButtonBase
            {...props}
            defaultStyle="text-gray-700"
            hoverStyle="hover:text-indigo-600">
        </ButtonBase>
    )
}

export default TopNavbarItem