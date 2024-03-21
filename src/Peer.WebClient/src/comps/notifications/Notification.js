import React from 'react'
import { Link } from 'react-router-dom'

const Notification = ({ notif }) => {
    if (notif.type == "NewApplicationReceived")
        return (
            <Link to="/applications/received">
                <div className='px-8 py-4 border-b flex flex-col gap-y-1 hover:bg-gray-50 cursor-pointer'>
                    <p className='text-gray-800'>{notif.message}</p>
                    <p className='text-gray-400 uppercase text-sm font-semibold tracking-wider'>{notif.when}</p>
                </div>
            </Link>
        )
}

export default Notification