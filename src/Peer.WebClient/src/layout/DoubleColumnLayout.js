import React from 'react'
import PageDescription from './PageDescription';
import PageTitle from './PageTitle';

export const DoubleColumnLayout = (props) => {
    const {
        title,
        description,
        children,
    } = props;

    return (
        <div className='absolute max-w-7xl mx-auto right-0 left-0 py-36 pb-16'>
            <div className='flex flex-col space-y-2'>
                <PageTitle title={title}></PageTitle>
                <PageDescription description={description}></PageDescription>
            </div>

            <div className='mt-16'>
                {children}
            </div>
        </div>
    )
}
