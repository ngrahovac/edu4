import React from 'react'
import { useAuth0 } from '@auth0/auth0-react'

const LogoutButton = () => {
    const { logout } = useAuth0();

    return (
        <button
            type="button"
            className='px-3 py-2 rounded-md bg-stone-200 hover:bg-stone-300 text-slate-800 font-semibold text-md ml-2'
            onClick={() => logout({ returnTo: window.location.origin })}>
            Log out
        </button>
    )
}

export default LogoutButton