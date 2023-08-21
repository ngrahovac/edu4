import React from 'react'

const LandingNavbar = (props) => {
    const {
        children
    } = props;

    return (
        <div
            className='fixed w-full h-16 flex flex-row items-center justify-end px-96 top-0 left-0 space-x-4'>
                {children}
        </div>
    )
}

export default LandingNavbar