import React from 'react'
import PageTitle from './PageTitle';
import PageDescription from './PageDescription';

const SingleColumnLayout = (props) => {

    const {
        title,
        description,
        children
    } = props;

    return (
        <div className='w-1/2 mx-auto relative pb-48 pt-36'>
            <div className='flex flex-col gap-y-2'>
                <PageTitle title={title}></PageTitle>
                <PageDescription description={description}></PageDescription>
            </div>

            <div className='my-16'>
                {children}
            </div>
        </div>
    )
}

export default SingleColumnLayout