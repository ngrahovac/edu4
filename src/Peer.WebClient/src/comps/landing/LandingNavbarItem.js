import React from 'react'

const LandingNavbarItem = (props) => {
    const {
        children,
        onClick = () => {}
    } = props;

    return (
        <p className='text-gray-700 hover:text-indigo-500 cursor-pointer py-2 font-semibold'>{children}</p>
    )
}

export default LandingNavbarItem