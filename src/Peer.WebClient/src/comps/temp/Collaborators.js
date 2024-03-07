import React from 'react'
import Gravatar from 'react-gravatar'

const Collaborators = ({ collaborations }) => {
    return (
        <div className='flex justify-end w-32 -space-x-6'>
            {
                collaborations &&

                collaborations.map(c => <div 
                    key={c.id}
                    title={c.collaboratorName}>
                    <Gravatar 
                        email={c.collaboratorEmail} 
                        className='rounded-full'
                        default='retro'></Gravatar>
                </div>)
            }
        </div>
    )
}

export default Collaborators