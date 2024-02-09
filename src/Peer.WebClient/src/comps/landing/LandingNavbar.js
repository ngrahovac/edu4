import React from 'react'

const LandingNavbar = (props) => {
    const {
        children
    } = props;

    return (
        <div
            className='fixed w-full top-0 left-0 h-16 bg-slate-50 flex place-content-center'>
            <div className='flex items-center justify-between w-5/6'>
                {children}
            </div>
        </div>
    )
}

export default LandingNavbar