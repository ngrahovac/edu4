import React from 'react'

const LandingNavbar = (props) => {
    const {
        children
    } = props;

    return (
        <div
            className='fixed top-0 w-2/3 h-16 flex items-center justify-between bg-white'>
                {children}
        </div>
    )
}

export default LandingNavbar