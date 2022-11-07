import React from 'react'
import { useAuth0 } from '@auth0/auth0-react'

const EnterButton = () => {
    const { loginWithRedirect } = useAuth0();

    return (
        <button
            type="button"
            className='px-3 py-2 rounded-md bg-stone-200 hover:bg-stone-300 text-slate-800 font-semibold text-md ml-2'
            onClick={() => loginWithRedirect()}>
            Enter
        </button>
    )
}

export default EnterButton