import React from 'react'

const LandingLayout = (props) => {
    const {
        children
    } = props;

    return (
        <div className='relative px-96 pt-16 flex flex-col h-screen'>
            {children}
        </div>
    )
}

export default LandingLayout