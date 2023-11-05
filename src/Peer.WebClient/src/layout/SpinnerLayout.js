import React from 'react'

const SpinnerLayout = (props) => {

    const {
        children
    } = props;

    return (
        <div className='absolute z-50 top-0 bottom-0 w-full bg-white flex flex-col place-content-center'>
            <div className='mx-auto'>
                {children}
            </div>
        </div>
    )
}

export default SpinnerLayout