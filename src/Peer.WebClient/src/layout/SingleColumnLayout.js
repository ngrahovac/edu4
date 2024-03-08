import React from 'react'
import PageTitle from './PageTitle';
import PageDescription from './PageDescription';
import Footer from './Footer';

const SingleColumnLayout = (props) => {

    const {
        title,
        description,
        children
    } = props;

    return (
        <div className='flex flex-col'>
            <div className='w-5/6 lg:w-1/2 mx-auto relative pb-48 pt-36 min-h-screen'>
                <div className='flex flex-col gap-y-2'>
                    <PageTitle title={title}></PageTitle>
                    <PageDescription description={description}></PageDescription>
                </div>

                <div className='my-8 mb-64'>
                    {children}
                </div>
            </div>

            <div className='w-full'>
                <Footer></Footer>
            </div>
        </div>
    )
}

export default SingleColumnLayout