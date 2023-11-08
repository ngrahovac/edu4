import React from 'react'
import ContributionsMenuItem from './ContributionsMenuItem'

const ContributionsMenu = () => {
    
    return (
        <div className='flex flex-col pl-4 pt-32'>
            <ContributionsMenuItem>My projects</ContributionsMenuItem>
            <ContributionsMenuItem>My collaborations</ContributionsMenuItem>
        </div>
    )
}

export default ContributionsMenu