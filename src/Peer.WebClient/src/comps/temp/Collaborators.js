import React from 'react'
import Gravatar from 'react-gravatar'

const Collaborators = ({ collaborators }) => {
    return (
        <div className='flex justify-end w-32 -space-x-6'>
            {
                collaborators &&

                collaborators.map(c => <div key={c.id}>
                    <Gravatar email={c.email} default='retro'></Gravatar>
                </div>)
            }
        </div>
    )
}

export default Collaborators