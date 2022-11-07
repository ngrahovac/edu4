import React from 'react'

const SingleColumnLayout = (props) => {

    const { title, children } = props;

    return (
        <div
            className='h-max w-5/6 md:w-2/3 lg:w-1/3 mx-auto'>
            <h1
                className='mt-28 mb-24 font-semibold text-4xl text-slate-800'>
                {title}
            </h1>

            <div
                className='relative flex flex-col bottom-0 h-max'>
                {children}
            </div>
        </div>
    )
}

export default SingleColumnLayout