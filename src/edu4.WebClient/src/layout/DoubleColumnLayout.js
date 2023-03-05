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
        <div
            className='w-2/3 mx-auto absolute mt-16 right-0 left-0 py-32'>
            <PageTitle title={title}></PageTitle>
            <PageDescription description={description}></PageDescription>

            <div className='flex flex-row mt-16'>
                {/* left pane */}
                <div className='w-1/2 mr-16'>
                    {left}
                </div>

                {/* right pane */}
                <div className='w-1/2 ml-16'>
                    {right}
                </div>
            </div>
        </div>
    )
}
