import React, { useEffect } from 'react'
import TopNavbarItem from './TopNavbarItem'
import { useState } from 'react';
import { getNotifs } from '../../services/NotificationsService';
import { useAuth0 } from '@auth0/auth0-react';
import { successResult, failureResult, errorResult } from '../../services/RequestResult'
import NotificationsPreview from '../notifications/NotificationsPreview';

const NotificationBell = () => {

    const [hasNotifs, setHasNotifs] = useState(false);
    const [notifs, setNotifs] = useState(undefined);
    const [previewVisible, setPreviewVisible] = useState(true);

    const { getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        const fetchNotifs = async () => {
            try {
                console.log('fetching notifs...')
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getNotifs(token);

                if (result.outcome === successResult) {
                    let notifs = result.payload;
                    setHasNotifs(notifs && notifs.length > 0);
                    setNotifs(notifs);
                } else if (result.outcome === failureResult) {
                    console.log("failure fetching notifs");
                }
            } catch (ex) {
                console.log("ex while fetching notifs", ex);
            } finally {

            }
        }
        const fetchNotifsInterval = setInterval(async () => await fetchNotifs(), 10000);

        return () => clearInterval(fetchNotifsInterval);
    }, [])

    function togglePreviewVisibility() {
        setPreviewVisible(!previewVisible);
    }

    return (
        <div className='relative'>
            <TopNavbarItem
                icon={
                    !hasNotifs ?
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M14.857 17.082a23.848 23.848 0 0 0 5.454-1.31A8.967 8.967 0 0 1 18 9.75V9A6 6 0 0 0 6 9v.75a8.967 8.967 0 0 1-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 0 1-5.714 0m5.714 0a3 3 0 1 1-5.714 0" />
                        </svg> :
                        <div className='relative'>
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M14.857 17.082a23.848 23.848 0 0 0 5.454-1.31A8.967 8.967 0 0 1 18 9.75V9A6 6 0 0 0 6 9v.75a8.967 8.967 0 0 1-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 0 1-5.714 0m5.714 0a3 3 0 1 1-5.714 0" />
                            </svg>
                            <span className="absolute top-0 right-0 inline-flex rounded-full h-3 w-3 bg-lime-500"></span>
                        </div>
                }
                onClick={togglePreviewVisibility}>
            </TopNavbarItem>

            <div className={`absolute top-8 right-8 ${!previewVisible && "hidden"}`}>
                <NotificationsPreview notifs={notifs}>
                </NotificationsPreview>
            </div>
        </div>
    )
}

export default NotificationBell