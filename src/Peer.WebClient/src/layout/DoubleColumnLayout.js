import React from 'react'
import PageDescription from './PageDescription';
import PageTitle from './PageTitle';
import Footer from './Footer';

export const DoubleColumnLayout = (props) => {
    const {
        title,
        description,
        left,
        right,
    } = props;

    return (
        <div className='flex flex-col'>
            <div className='mx-auto w-5/6 lg:w-3/4 pb-48 pt-36 min-h-screen'>
                <div className='flex flex-col gap-y-2'>
                    <PageTitle title={title}></PageTitle>
                    <PageDescription description={description}></PageDescription>
                </div>

                <div className='my-8 mb-64 flex flex-col gap-y-8 pt-16 lg:flex-row lg:gap-x-12'>
                    {left}
                    {right}
                </div>
            </div>

            <div className='w-full'>
                <Footer></Footer>
            </div>
        </div>
    )
}
