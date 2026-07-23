

"use client";

import { getAsync } from "@/services/apiHandler";
import { use, useEffect, useState } from "react";


export default function userDetails() {
    const [name, setName] = useState("");

    const fetchUserDetails = async () => {
        try {
           const response = await fetch("https://localhost:7287/api/User/list", {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                }
            });
            debugger
             const users = await response.json();
          if (users.length > 0) {
            
            setName(users.Data[0].username);
          }
          return users;
              

        } catch (error) {
            alert("Error fetching user details:");
        }
    }

    useEffect(() => {
        setName("John Doe");
         fetchUserDetails().then((data : any) => {
            debugger
             setName(data.data[0].username);
        });
    }, [])


    return (


        <div className="login-page-container">
            <div className="glow-orb-1"></div>
            <div className="glow-orb-2"></div>
            <p>User Details</p>
            <p>{name}</p>
            <div className=" flex flex-col mt-4 input-container">

                <br />
                <input type="text"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
            </div>
        </div>
    );
}
