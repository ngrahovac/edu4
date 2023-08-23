import React from 'react'
import PageDescription from './PageDescription';
import PageTitle from './PageTitle';

export const DoubleColumnLayout = (props) => {
    const {
        title,
        description,
        right,
        left
    } = props;

    return (
        <div className='absolute max-w-7xl mx-auto right-0 left-0 py-36'>
            <div className='flex flex-col space-y-2'>
                <PageTitle title={title}></PageTitle>
                <PageDescription description={description}></PageDescription>
            </div>

            <div className='flex flex-row gap-x-24 mt-16'>
                {/* left pane */}
                <div className='w-full'>
                    {left}
                </div>

                {/* right pane */}
                <div className='w-full'>
                    {right}
                </div>
            </div>
        </div>
    )
}
