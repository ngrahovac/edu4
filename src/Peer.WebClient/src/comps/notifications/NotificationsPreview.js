import React from 'react'
import Notification from './Notification'

const NotificationsPreview = ({ notifs }) => {
    return (
        <div className='flex flex-col w-80 max-h-96 drop-shadow-xl border border-gray-200 bg-white rounded-tr-none gap-y-1'>
            {
                notifs &&
                notifs.map(n => <Notification notif={n}></Notification>)
            }
        </div>
    )
}

export default NotificationsPreview