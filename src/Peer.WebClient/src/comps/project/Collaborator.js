import React from 'react'
import Gravatar from 'react-gravatar'
import { Link } from 'react-router-dom'

const Collaborator = ({ collaborator, position }) => {

    return (
        <div className='flex shrink-0 gap-x-4 items-center'>
            <Gravatar email={collaborator.email} className='rounded-full'></Gravatar>

            <div className='flex flex-col'>
                <Link to={`/contributors/${collaborator.id}`}>
                    <p className='hover:text-indigo-400 cursor-pointer font-semibold text-lg text-gray-700'>{collaborator.fullName}</p>

                </Link>
                <p className='text-gray-500'>{position}</p>
            </div>
        </div>
    )
}

export default Collaborator