import React from 'react'

const LandingLayout = (props) => {
    const {
        children
    } = props;

    return (
        <div className='w-5/6 mx-auto h-screen'>
            {children}
        </div>
    )
}

export default LandingLayout