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
        <div
            className='w-5/6 md:w-2/3 lg:w-1/2 mx-auto absolute mt-16 right-0 left-0 py-32'>
            <PageTitle title={title}></PageTitle>
            <PageDescription description={description}></PageDescription>

            {children}
        </div>
    )
}

export default SingleColumnLayout