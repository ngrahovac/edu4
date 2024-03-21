import React from 'react'
import Notification from './Notification'

const NotificationsPreview = ({ notifs }) => {
    return (
        <div className='flex flex-col w-80 max-h-96 shadow-sm drop-shadow-lg bg-white rounded-tr-none gap-y-1'>
            {
                notifs &&
                notifs.map(n => <Notification notif={n}></Notification>)
            }

            {
                !notifs &&
                <p className='px-4 py-2'>No new notifications.</p>
            }
        </div>
    )
}

export default NotificationsPreview