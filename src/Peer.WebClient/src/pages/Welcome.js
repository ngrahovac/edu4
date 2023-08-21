import React from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'

const Welcome = () => {
    return (
        <SingleColumnLayout
            title="Welcome &#x1F49C;">

            <SectionTitle
                title="What's new">
            </SectionTitle>

            <ul className='list-disc mt-4 ml-4'>
                <li>The platform is in private beta! Take a look around, explore, and reach out with <span className='font-bold'>ANY and ALL</span> feedback!</li>
            </ul>
        </SingleColumnLayout>
    )
}

export default Welcome